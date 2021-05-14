FROM nginx:alpine
WORKDIR /var/www

COPY docker/nginx/prod-nginx.conf /etc/nginx/nginx.conf
COPY docker/nginx/sites/prod.conf /etc/nginx/sites-available/prod.conf
COPY docker/nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf
COPY docker/nginx-host/additional/options-ssl-nginx.conf /etc/ssl/options-ssl-nginx.conf
COPY docker/nginx-host/additional/ssl-dhparams.pem /etc/ssl/ssl-dhparams.pem

CMD nginx
EXPOSE 443 80