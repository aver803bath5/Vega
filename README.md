 # Vega
 
該專案可以 CRUD Vehicle 的資料，上傳與刪除 Vehicle 的照片。

使用 ASP .NET Core 搭建後端 API，Angular 搭建前端，並使用 Auth0 實現登入與 API 身分驗證的功能。並且使用 docker-compose 來管理開發用的 docker 環境。

## Development Setup

1. 安裝 docker 以及 docker-compose
2. 將 `appsettings.sample.json` 的內容依據裡面的提示填好，之後將檔名改成 `appsettings.Development.json`。
3. 將 `ClientApp/auth_config.sample.json` 的內容依據檔案內容提示填好後將檔名改成 `auth_config.json`。
4. 將 `docker-compose.yml` 內的 `api` service bind 給 container 內 `/var/www/html/uploads` 路徑的 host 路徑改成開發機器要儲存上傳圖片的資料夾路徑，該路徑會跟第一項中建立的 `appsettings.Development.json` 中的 `StoredFilePath` property 的值相同。
5. 實行 `docker-compose up -d` 就能夠開啟開發用的環境了。
