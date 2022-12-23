FROM nginx:1.19-alpine
LABEL author="Rolling Glory <hello@rollingglory.com>" \
    name="NFT Game" \
    version="1.0"

ENV TZ Asia/Jakarta
ARG WEBFOLDER=nftgame
ENV WEBFOLDER=${WEBFOLDER}

RUN apk add --no-cache tzdata && \
    cp /usr/share/zoneinfo/${TZ} /etc/localtime &&\
    echo ${TZ} > /etc/timezone

COPY Config/nginx.conf /etc/nginx/nginx.conf
COPY Config/default.conf /etc/nginx/conf.d/default.conf
COPY ${WEBFOLDER}/ /usr/share/nginx/html/

USER nginx

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
