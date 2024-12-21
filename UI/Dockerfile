FROM node:16

WORKDIR /app

COPY ./package.json .
COPY . .

RUN npm install
RUN npm install react-hook-form

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host"]
