#!/usr/bin/env bash
set -euo pipefail

source /etc/os-release

sudo apt-get update
sudo apt-get install -y wget apt-transport-https ca-certificates

if ! apt-cache show aspnetcore-runtime-8.0 >/dev/null 2>&1; then
  wget "https://packages.microsoft.com/config/ubuntu/${VERSION_ID}/packages-microsoft-prod.deb" -O /tmp/packages-microsoft-prod.deb
  sudo dpkg -i /tmp/packages-microsoft-prod.deb
  rm /tmp/packages-microsoft-prod.deb
  sudo apt-get update
fi

sudo apt-get install -y aspnetcore-runtime-8.0 nginx

sudo mkdir -p /var/www/propertease/releases /var/www/propertease/data /var/www/propertease/keys /etc/propertease
sudo chown -R www-data:www-data /var/www/propertease

if [ ! -f /etc/propertease/propertease.env ]; then
  sudo tee /etc/propertease/propertease.env >/dev/null <<'ENV'
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://127.0.0.1:5000
Database__Provider=Sqlite
Database__EnsureCreated=true
Database__AutoMigrate=false
ConnectionStrings__DefaultConnection=Data Source=/var/www/propertease/data/propertease.db
DataProtection__KeysPath=/var/www/propertease/keys
Security__UseHttpsRedirection=false
Security__UseHsts=false
SeedAdmin__Email=admin@propertease.com
SeedAdmin__Password=CHANGE_ME_BEFORE_FIRST_START
ENV
fi

sudo chmod 600 /etc/propertease/propertease.env
sudo chown root:root /etc/propertease/propertease.env

echo "Bootstrap complete. Copy deploy/propertease.service to /etc/systemd/system/ and deploy/propertease.nginx to /etc/nginx/sites-available/propertease during first deployment."
