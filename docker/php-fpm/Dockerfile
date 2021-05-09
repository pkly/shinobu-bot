FROM php:8.0.6-fpm-alpine3.13

# dependencies
RUN apk update
RUN apk add zlib-dev icu-dev g++ curl curl-dev freetype-dev \
    libxml2-dev autoconf imagemagick-dev build-base bash \
    file coreutils ncurses oniguruma-dev libzip-dev

# install generic extensions
RUN docker-php-ext-install pdo_mysql
RUN docker-php-ext-install mbstring
RUN docker-php-ext-configure intl
RUN docker-php-ext-install intl
RUN docker-php-ext-install curl
RUN docker-php-ext-install sockets
RUN docker-php-ext-install bcmath
RUN docker-php-ext-install opcache

# install redis
RUN pecl install -o -f redis \
&&  rm -rf /tmp/pear \
&&  docker-php-ext-enable redis

# install composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# must be hard-copied because of build context
COPY ./docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh
ENTRYPOINT ["/usr/bin/docker-entrypoint.sh"]

WORKDIR /var/www
CMD php-fpm
EXPOSE 9000
