FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /home/bot
COPY Shinobu app
RUN chmod +x app
COPY bot/assets assets
CMD ls -la && ./app