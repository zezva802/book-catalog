#!/usr/bin/env bash
# Seeds the in-memory catalog with books that exercise the list filters.
# Usage: start the API, then run ./scripts/seed-books.sh
#        Override the URL with: BASE=http://localhost:1234 ./scripts/seed-books.sh
set -euo pipefail

BASE="${BASE:-http://localhost:5155}"

post() {
  curl -s -o /dev/null -w "%{http_code}  $1\n" \
    -X POST "$BASE/api/books" \
    -H "Content-Type: application/json" \
    -d "$1"
}

echo "Seeding $BASE ..."

# Two titles containing "Dune" -> tests title filter and sorting
post '{"title":"Dune","author":"Frank Herbert","year":1965}'
post '{"title":"Dune Messiah","author":"Frank Herbert","year":1969}'

# Exact duplicate title -> tests the Title-then-Id tiebreak
post '{"title":"Dune","author":"Frank Herbert","year":1965}'

# BCE year (negative)
post '{"title":"The Odyssey","author":"Homer","year":-700}'

# No author, BCE -> author filter must exclude it without crashing
post '{"title":"The Epic of Gilgamesh","year":-2100}'

# No author AND no year -> excluded once yearFrom/yearTo is set
post '{"title":"One Thousand and One Nights"}'

post '{"title":"Vepkhistkaosani","author":"Shota Rustaveli","year":1200}'
post '{"title":"The Hobbit","author":"J.R.R. Tolkien","year":1937}'

# Intentionally invalid (whitespace-only author) -> expect 400
post '{"title":"Invalid Book","author":"   ","year":2000}'

echo "Done."