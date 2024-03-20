FROM node:20-alpine as build

WORKDIR /app

COPY package*.json /app/

RUN npm install

COPY . /app

RUN npm install -g @angular/cli

CMD ["ng", "serve", "--host", "0.0.0.0"]
