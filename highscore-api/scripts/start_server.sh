#!/bin/bash
export RAILS_ENV=production
export RAILS_SERVE_STATIC_FILES=true
cd /home/ubuntu/app/highscore-api
source ~/.rvm/scripts/rvm
rvm use 3.3.5
bundle exec rails server -p 3000 -b 0.0.0.0
