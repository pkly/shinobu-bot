FROM nginx:alpine
WORKDIR /var/www

COPY docker/nginx/prod-nginx.conf /etc/nginx/nginx.conf
COPY docker/nginx/sites/prod.conf /etc/nginx/sites-available/prod.conf
COPY docker/nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf

CMD nginx
EXPOSE 80