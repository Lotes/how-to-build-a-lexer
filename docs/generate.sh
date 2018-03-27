#!/bin/bash
FILES=*.gv
for source in $FILES
do
  target=$source.png
  dot -Tpng -o$target $source
  echo "Generated:  "$target
done