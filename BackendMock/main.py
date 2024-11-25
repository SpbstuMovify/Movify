from fastapi import FastAPI, HTTPException, Depends, Header, UploadFile, File
from typing import List, Optional
from pydantic import BaseModel, Field
from uuid import UUID, uuid4
from fastapi.responses import FileResponse

app = FastAPI()

# In-memory data structures to simulate a database
contents = {}
episodes = {}
users = {}
tokens = {}
buckets = {}

# Token validation dependency
def token_validation(Authorization: Optional[str] = Header(None)):
    if not Authorization or Authorization not in tokens.values():
        raise HTTPException(status_code=403, detail="Требуется авторизация")

# Models
class ErrorResponse(BaseModel):
    code: int
    error: str
    errorDescription: Optional[str] = None

class CastMemberDto(BaseModel):
    employeeFullName: Optional[str] = None
    roleName: Optional[str] = None

class CreateContentDto(BaseModel):
    title: Optional[str] = None
    quality: Optional[str] = None
    genre: Optional[str] = None
    category: Optional[str] = None
    ageRestriction: Optional[str] = None
    description: Optional[str] = None
    thumbnail: Optional[str] = None
    remainingTime: Optional[str] = None
    castMembers: Optional[List[CastMemberDto]] = None

class UpdateContentDto(CreateContentDto):
    pass

class CreateEpisodeDto(BaseModel):
    number: int
    title: Optional[str] = None
    description: Optional[str] = None
    s3BucketName: Optional[str] = None
    contentId: UUID

class UpdateEpisodeDto(BaseModel):
    number: Optional[int] = None
    title: Optional[str] = None
    description: Optional[str] = None
    s3BucketName: Optional[str] = None
    contentId: Optional[UUID] = None

class LoginDto(BaseModel):
    email: Optional[str] = None
    password: Optional[str] = None

class RegisterDto(BaseModel):
    email: Optional[str] = None
    password: Optional[str] = None
    isAdmin: Optional[bool] = None

class TokenDto(BaseModel):
    token: str

class BucketDto(BaseModel):
    bucketName: str

class FileDto(BaseModel):
    bucketName: str
    key: str
    prefix: Optional[str] = None

# Routes

# Auth Service API

@app.post("/api/v1/login", response_model=TokenDto)
def login(login_data: LoginDto):
    for user_id, user in users.items():
        if user['email'] == login_data.email and user['password'] == login_data.password:
            token = f"token-{uuid4()}"
            tokens[user_id] = token
            return {"token": token}
    raise HTTPException(status_code=400, detail="Некорректный запрос")

@app.post("/api/v1/register", response_model=TokenDto)
def register(register_data: RegisterDto):
    user_id = str(uuid4())
    users[user_id] = {
        "id": user_id,
        "email": register_data.email,
        "password": register_data.password,
        "isAdmin": register_data.isAdmin or False
    }
    token = f"token-{uuid4()}"
    tokens[user_id] = token
    return {"token": token}

# Content Service API

@app.get("/api/v1/content/search")
def search_content(
    query: Optional[str] = None,
    title: Optional[str] = None,
    genre: Optional[str] = None,
    category: Optional[str] = None,
    age_restriction: Optional[str] = None,
    release_date: Optional[str] = None,
    page: int = 1,
    limit: int = 10,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    content_list = list(contents.values())

    # Apply filters
    if query:
        content_list = [c for c in content_list if query.lower() in (c.get('title') or '').lower()]
    if title:
        content_list = [c for c in content_list if title.lower() in (c.get('title') or '').lower()]
    if genre:
        content_list = [c for c in content_list if genre.lower() == (c.get('genre') or '').lower()]
    if category:
        content_list = [c for c in content_list if category.lower() == (c.get('category') or '').lower()]
    if age_restriction:
        content_list = [c for c in content_list if age_restriction == c.get('ageRestriction')]
    # For simplicity, release_date filtering is omitted

    # Pagination
    start = (page - 1) * limit
    end = start + limit
    return content_list[start:end]

@app.get("/api/v1/content")
def get_all_content(Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    return list(contents.values())

@app.post("/api/v1/content")
def create_content(
    content_data: CreateContentDto,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    content_id = str(uuid4())
    content_dict = content_data.dict()
    content_dict['id'] = content_id
    contents[content_id] = content_dict
    return content_dict

@app.get("/api/v1/content/{id}")
def get_content(id: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    content = contents.get(id)
    if not content:
        raise HTTPException(status_code=404, detail="Content not found")
    return content

@app.delete("/api/v1/content/{id}")
def delete_content(id: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    if id in contents:
        del contents[id]
        return {"detail": "Content deleted"}
    else:
        raise HTTPException(status_code=404, detail="Content not found")

@app.put("/api/v1/content/{id}")
def update_content(
    id: str,
    content_data: UpdateContentDto,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if id in contents:
        updated_content = contents[id]
        updated_content.update(content_data.dict(exclude_unset=True))
        contents[id] = updated_content
        return updated_content
    else:
        raise HTTPException(status_code=404, detail="Content not found")

@app.get("/api/v1/content/{id}/episode")
def get_content_episodes(id: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    content = contents.get(id)
    if not content:
        raise HTTPException(status_code=404, detail="Content not found")
    content_episodes = [ep for ep in episodes.values() if str(ep['contentId']) == id]
    return content_episodes

@app.post("/api/v1/episode")
def create_episode(
    episode_data: CreateEpisodeDto,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    episode_id = str(uuid4())
    episode_dict = episode_data.dict()
    episode_dict['id'] = episode_id
    episodes[episode_id] = episode_dict
    return episode_dict

@app.delete("/api/v1/episode/{id}")
def delete_episode(id: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    if id in episodes:
        del episodes[id]
        return {"detail": "Episode deleted"}
    else:
        raise HTTPException(status_code=404, detail="Episode not found")

@app.get("/api/v1/episode/{id}")
def get_episode(id: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    episode = episodes.get(id)
    if not episode:
        raise HTTPException(status_code=404, detail="Episode not found")
    return episode

@app.put("/api/v1/episode/{id}")
def update_episode(
    id: str,
    episode_data: UpdateEpisodeDto,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if id in episodes:
        updated_episode = episodes[id]
        updated_episode.update(episode_data.dict(exclude_unset=True))
        episodes[id] = updated_episode
        return updated_episode
    else:
        raise HTTPException(status_code=404, detail="Episode not found")

@app.delete("/api/v1/users/{login}")
def delete_user(login: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    user = next((u for u in users.values() if u['email'] == login), None)
    if user:
        user_id = user['id']
        del users[user_id]
        tokens.pop(user_id, None)
        return {"detail": "User deleted"}
    else:
        raise HTTPException(status_code=404, detail="User not found")

@app.get("/api/v1/users/{login}")
def get_user(login: str, Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    user = next((u for u in users.values() if u['email'] == login), None)
    if user:
        return user
    else:
        raise HTTPException(status_code=404, detail="User not found")

@app.put("/api/v1/users/{login}")
def update_user(
    login: str,
    user_data: RegisterDto,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    user = next((u for u in users.values() if u['email'] == login), None)
    if user:
        user_id = user['id']
        users[user_id].update(user_data.dict(exclude_unset=True))
        return users[user_id]
    else:
        raise HTTPException(status_code=404, detail="User not found")

# Media Service API

@app.post("/api/v1/buckets")
def create_bucket(
    bucket_name: str,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if bucket_name in buckets:
        raise HTTPException(status_code=400, detail="Bucket already exists")
    buckets[bucket_name] = {}
    return {"bucketName": bucket_name}

@app.get("/api/v1/buckets")
def list_buckets(Authorization: Optional[str] = Header(None)):
    token_validation(Authorization)
    return [{"bucketName": name} for name in buckets.keys()]

@app.delete("/api/v1/buckets/{bucket_name}")
def delete_bucket(
    bucket_name: str,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if bucket_name in buckets:
        del buckets[bucket_name]
        return {"detail": "Bucket deleted"}
    else:
        raise HTTPException(status_code=400, detail="Bucket not found")

@app.post("/api/v1/buckets/{bucket_name}/files")
def upload_file(
    bucket_name: str,
    proc_video: bool,
    file: UploadFile = File(...),
    prefix: Optional[str] = None,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if bucket_name not in buckets:
        raise HTTPException(status_code=400, detail="Bucket not found")
    file_key = f"{prefix}/{file.filename}" if prefix else file.filename
    # File saving logic is not required, so we'll just simulate it
    buckets[bucket_name][file_key] = {
        "bucketName": bucket_name,
        "key": file_key,
        "prefix": prefix
    }
    return {
        "bucketName": bucket_name,
        "key": file_key,
        "prefix": prefix
    }

@app.get("/api/v1/buckets/{bucket_name}/files")
def list_files(
    bucket_name: str,
    prefix: Optional[str] = None,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if bucket_name not in buckets:
        raise HTTPException(status_code=400, detail="Bucket not found")
    files = buckets[bucket_name].values()
    if prefix:
        files = [f for f in files if f.get('prefix') == prefix]
    return list(files)

@app.delete("/api/v1/buckets/{bucket_name}/files/{key}")
def delete_file(
    bucket_name: str,
    key: str,
    Authorization: Optional[str] = Header(None)
):
    token_validation(Authorization)
    if bucket_name not in buckets or key not in buckets[bucket_name]:
        raise HTTPException(status_code=400, detail="File not found")
    del buckets[bucket_name][key]
    return {"detail": "File deleted"}

@app.get("/api/v1/buckets/{bucket_name}/files/{key}")
def get_file(
    bucket_name: str,
    key: str
):
    # This endpoint does not require authorization
    if bucket_name not in buckets or key not in buckets[bucket_name]:
        raise HTTPException(status_code=400, detail="File not found")
    # Return a static local file
    file_path = "static/sample_file.txt"  # Path to a static local file
    return FileResponse(file_path, media_type='application/octet-stream', filename=key)
