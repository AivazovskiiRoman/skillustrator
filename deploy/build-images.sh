#!/bin/bash
set -e -x

ENV_SUFFIX="-dev"
if [ "$TRAVIS_BRANCH" == "master" ]; then 
  ENV_SUFFIX="-dev"
elif [ "$TRAVIS_BRANCH" == "staging" ]; then 
  ENV_SUFFIX="-stg"
elif [ "$TRAVIS_BRANCH" == "production" ]; then 
  ENV_SUFFIX="-prod"
fi

# docker ps 
# docker-compose -f docker-compose$ENV_SUFFIX.yml build
# docker-compose -f docker-compose$ENV_SUFFIX.yml up -d && docker ps


docker-compose build --pull

# docker-compose run excella-retro npm run lint
# docker-compose run ng test --browsers Chrome_no_sandbox -w false
# docker-compose run npm run e2e

docker-compose run ui npm cache clean
docker-compose run ui ng build --prod

#if [ "$TRAVIS_BRANCH" == "production" ]; then
#docker-compose -f docker-compose-prod.yml up -d
#fi
