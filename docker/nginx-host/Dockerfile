FROM nginx:alpine
WORKDIR /var/www

COPY ./nginx/prod-nginx.conf /etc/nginx/nginx.conf
COPY ./nginx/sites/prod.conf /etc/nginx/sites-available/prod.conf
COPY ./nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf

CMD nginx
EXPOSE 443