FROM node:16

WORKDIR /app

COPY ./package.json .
COPY . .

RUN npm install
RUN npm install react-hook-form
RUN npm install jwt-decode

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host"]
