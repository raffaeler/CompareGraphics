#!/bin/bash
LOCALFOLDER="./bin/Release/net6.0/linux-arm/publish/."
REMOTEFOLDER="fonttest"
EXE="ConsoleTest"
USER="pi"
REMOTE="intercom4"

FULLREMOTE="${USER}@${REMOTE}:~/${REMOTEFOLDER}"
# add "--delete" to rsync to "delete extraneous files from destination dirs"

RED="\033[0;31m"
GREEN="\033[0;32m"
YELLOW="\033[1;33m"
NC="\033[0m" # No Color

chmod +x ${LOCALFOLDER}/${EXE}

printf "${YELLOW}Press ESC to exit or any other key to sync folder \"${PWD##*/}\" ${NC}\n"
while read -rs -n 1 key; do
  if [[ $key == $'\e' ]]; then
    break;
  fi

  dt=$(date '+%d/%m/%Y %H:%M:%S');
  printf "${GREEN}copying ...' ${dt} ${NC}\n"
  rsync -avhzrE --progress -e ssh ${LOCALFOLDER} ${FULLREMOTE}
  printf "${GREEN}end of copy ${dt} ${NC}\n"
  printf "${YELLOW}Press any key to sync ${pwd} ${NC}\n"
done

