FROM mcr.microsoft.com/dotnet/sdk:5.0

# Required inside Docker, otherwise file=change events may not trigger.
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
ENV ASPNETCORE_ENVIRONMENT=Development

# Install libgdiplus pacakge to make System.Draw.Common package work.
RUN apt update
RUN apt install -y apt-utils
RUN apt install -y libgdiplus
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll

RUN ["mkdir", "-p", "/var/www/html/uploads"]

COPY . /app
WORKDIR /app

RUN ["dotnet", "restore"]
RUN ["dotnet", "tool", "restore"]
RUN ["dotnet", "build"]

RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh