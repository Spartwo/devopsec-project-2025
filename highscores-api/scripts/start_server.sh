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

# Change to application directory
cd /home/ubuntu/app/highscores-api
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
bundle check || bundle install || { log "Failed to install gems"; exit 1; }

# Precompile assets if needed
if [ ! -d "public/assets" ] || [ ! -f "public/assets/manifest.json" ]; then
  log "Precompiling assets..."
  bundle exec rake assets:precompile || log "Asset precompilation failed, but continuing..."
fi

# Run pending migrations if any
log "Running pending migrations..."
bundle exec rake db:migrate || log "Migrations failed, but continuing..."

# Start the Rails server
log "Starting Rails server on port 3000..."
exec bundle exec rails server -p 3000 -b 0.0.0.0
