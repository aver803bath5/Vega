FROM node:14.16.0

# Set working directory
WORKDIR /app

# Add `/app/node_modules/.bin` to $PATH, so we can run `ng serve` directly.
ENV PATH /app/node_modules/.bin:$PATH

# Install and cache app dependencies
COPY package.json /app/package.json
RUN npm install
RUN npm install -g @angular/cli@11.2.5

# Add app
COPY . /app

# Start app. Use 0.0.0.0 as the ip so I can access the app outside the container localhost.
CMD ng serve --host 0.0.0.0
