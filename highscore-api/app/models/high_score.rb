class HighScore < ApplicationRecord
  validates :name, presence: true, length: { maximum: 50 }
  validates :game, presence: true
  validates :score, presence: true, numericality: { greater_than_or_equal_to: 0 }
end
