package com.polytech.contentservice.service;

import com.polytech.contentservice.dto.ContentDto;
import com.polytech.contentservice.dto.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Set;
import java.util.UUID;
import java.util.stream.Collectors;

import static com.polytech.contentservice.mapper.ContentMapper.patchUpdate;

@Service
@RequiredArgsConstructor
public class ContentServiceImpl implements ContentService {
    private final ContentRepository contentRepository;

    @Override
    public ContentDto createContent(ContentDto contentDto) {
        return ContentMapper.convertToContentDto(contentRepository.save(ContentMapper.convertToContentEntity(contentDto)));
    }

    @Override
    @Transactional
    public void updateContent(UUID id, ContentDto contentDto) {
        Content oldContent = getContentById(id);
        Content updatedContent = patchUpdate(oldContent, contentDto);
        contentRepository.save(updatedContent);
    }

    @Override
    public Set<EpisodeDto> findAllEpisodesForContent(UUID contentId) {
        Content curContent = getContentById(contentId);
        return curContent.getEpisodes()
                .stream()
                .map(EpisodeMapper::convertToEpisodeDto)
                .collect(Collectors.toSet());
    }

    private Content getContentById(UUID contentId) {
        return contentRepository.findById(contentId)
            .orElseThrow(() -> new IllegalArgumentException("Episode not found"));
    }

    @Override
    public List<ContentDto> findAllContent(Pageable pageable) {
        return ContentMapper.convertToListOfContentDto(contentRepository.findAll(pageable));
    }

    @Override
    public ContentDto findContentById(UUID id) {
        return ContentMapper.convertToContentDto(getContentById(id));
    }
}
