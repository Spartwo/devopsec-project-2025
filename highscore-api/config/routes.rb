Rails.application.routes.draw do
  get '/up', to: proc { [200, {}, ['OK']] }
  
  get '/high_scores/top', to: 'high_scores#top_scores'
  post '/high_scores', to: 'high_scores#create'
  put '/high_scores/:id', to: 'high_scores#update'
  delete '/high_scores/:id', to: 'high_scores#destroy'
end
