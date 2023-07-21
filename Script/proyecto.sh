#!/bin/bash

report() {
  cd Informe
  if [ -z "$1" ]
  then
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape report.tex </dev/null
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape report.tex </dev/null
  else
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape report.tex </dev/null
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape report.tex </dev/null
  fi   
  cd ..
}

slides() {
  cd Presentacion
  if [ -z "$1" ]
  then
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentation.tex </dev/null
    pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentation.tex </dev/null
  else
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentation.tex </dev/null
    $1  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape presentation.tex </dev/null
  fi
  cd ..
}

clean() {
  cd Informe
  rm -f report.aux report.fls report.log report.fdb_latexmk report.out report.synctex.gz
  cd ..
  cd Presentacion
  rm -f presentation.aux presentation.fls presentation.log presentation.nav presentation.synctex.gz presentation.snm presentation.toc presentation.fdb_latexmk
  cd ..
  cd MoogleServer
  rm -r bin obj
  cd ..
  cd MoogleEngine
  rm -r bin obj
  cd ..
}

all_docs() {
  report;
  slides;
  cp Informe/report.pdf report.pdf
	cp Presentacion/presentation.pdf presentation.pdf
  clean;
}

veryclean() {
	clean;
  rm -f 
  rm -f Presentacion/presentation.pdf Informe/report.pdf report.pdf presentation.pdf 
}

show_report() {
  if [ ! -f  "Informe/report.pdf" ]
  then 
    report;
  fi

  if [ -z "$1" ]
  then
    xdg-open Informe/report.pdf
  else
    $1 Informe/report.pdf
  fi
}

show_slides() {
  if [ ! -f  "Presentacion/presentation.pdf" ]
  then
    slides;
  fi
   
  if [ -z "$1" ]
  then
    xdg-open Presentacion/presentation.pdf
  else
    $1 Presentacion/presentation.pdf
  fi  
}

run() {
  xdg-open https://localhost:5001/
  dotnet watch run --project MoogleServer
}

build() {
  dotnet build
}

help() {
  echo "This script builds Moogle and its documentation"
  echo ""
  echo "build       - to build Moogle"
  echo "run         - to run Moogle"
  echo "report      - to build Moogle's report"
  echo "slides      - to build Moogle's slides"
  echo "clean       - to clean up the latex's build process"
  echo "veryclean   - to clean up all documentation and all unnecessary files"
  echo "show_report - to show Moogle's report"
  echo "show_slides - to show Moogle's slides"
  echo "all_docs    - to build all Moogle's documentation and clean up all unnecessary files"
  echo "help        - to show this information"
  echo ""
}

cd ..
"$@"