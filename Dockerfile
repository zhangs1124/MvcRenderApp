# ==============================================================================
# 第一階段：SDK 建置環境 (Build Stage)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 csproj 並先還原快取依賴
COPY ["MvcRenderApp.csproj", "./"]
RUN dotnet restore "MvcRenderApp.csproj"

# 複製所有原始碼並進行 Release 發布
COPY . .
RUN dotnet publish "MvcRenderApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==============================================================================
# 第二階段：輕量化執行環境 (Runtime Stage)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 複製發布產物
COPY --from=build /app/publish .

# 建立 SQLite 資料存放目錄並配置 app 使用者權限
RUN mkdir -p /app/data && chown -R app:app /app

# 切換至安全非 root 使用者
USER app

# 設定預設連接埠 (Render 運行時會注入 $PORT 環境變數)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# 支援 Render 動態 PORT 綁定
ENTRYPOINT ["sh", "-c", "exec dotnet MvcRenderApp.dll --urls http://0.0.0.0:${PORT:-8080}"]
