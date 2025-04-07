#!/bin/bash
set -e

# Log function for better debugging
log() {
  echo "[$(date +'%Y-%m-%d %H:%M:%S')] $1" | tee -a /var/log/highscores-api/application.log
}

log "Starting Highscore API server..."

# Set environment variables
export RAILS_ENV=production
export RAILS_SERVE_STATIC_FILES=true
export RAILS_LOG_TO_STDOUT=true
export RAILS_MASTER_KEY=$(cat /home/ubuntu/app/Project-2025/highscores-api/config/master.key)

# Change to application directory
cd /home/ubuntu/app/Project-2025/highscores-api
log "Working directory: $(pwd)"

# Load RVM
log "Loading RVM..."
source ~/.rvm/scripts/rvm || { log "Failed to load RVM"; exit 1; }

# Use Ruby 3.3.5
log "Setting Ruby version..."
rvm use 3.3.5 || { log "Failed to set Ruby version"; exit 1; }
log "Using Ruby $(ruby -v)"

# Check if bundle is available
if ! command -v bundle &> /dev/null; then
  log "Bundler not found, installing..."
  gem install bundler || { log "Failed to install bundler"; exit 1; }
fi

# Check if all gems are installed
log "Checking bundle installation..."
bundle config set frozen false
bundle check || bundle install --without development test || { log "Failed to install gems"; exit 1; }

# Create storage directory if it doesn't exist
log "Ensuring storage directory exists..."
mkdir -p storage
chmod -R 755 storage

# Precompile assets if needed
if [ ! -d "public/assets" ] || [ ! -f "public/assets/manifest.json" ]; then
  log "Precompiling assets..."
  bundle exec rake assets:precompile || log "Asset precompilation failed, but continuing..."
fi

# Run pending migrations if any
log "Running database migrations..."
bundle exec rake db:migrate || log "Migrations failed, but continuing..."

# Check if database is seeded, if not seed it
if [ ! -f "storage/production.sqlite3" ] || [ ! -s "storage/production.sqlite3" ]; then
  log "Seeding database..."
  bundle exec rake db:seed || log "Database seeding failed, but continuing..."
fi

# Start the Rails server
log "Starting Rails server on port 3000..."
exec bundle exec rails server -p 3000 -b 0.0.0.0
