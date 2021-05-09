FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /home/bot
COPY Shinobu app
RUN chmod +x app
COPY bot/assets assets

COPY ../docker-entrypoint.sh /usr/bin/docker-entrypoint.sh
RUN chmod +x /usr/bin/docker-entrypoint.sh
ENTRYPOINT ["/usr/bin/docker-entrypoint.sh"]

CMD ./app