FROM ghcr.io/pkly/shinobu-bot/shinobu-php-fpm:latest

WORKDIR /var/www

COPY . /var/www

# remove the docker files from image since it's just trash here
RUN rm -rf /var/www/docker
RUN bin/console cache:warmup --env=prod
RUN bin/console cache:warmup --env=dev

RUN chown www-data:www-data /var/www/var
RUN chmod -R 755 /var/www/var/cache

CMD php-fpm
EXPOSE 9000
