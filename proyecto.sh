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
  rm -f report.aux report.fls report.log report.fdb_latexmk report.out report.synctex.gz report.toc report.pdf
  cd ..
  cd Presentacion
  rm -f presentation.aux presentation.fls presentation.log presentation.nav presentation.synctex.gz presentation.snm presentation.toc presentation.fdb_latexmk presentation.pdf
  cd ..
  cd MoogleServer
  
  if [ -r  "bin" ]
  then
    rm -r bin obj
  fi
  
  cd ..
  cd MoogleEngine
  if [ -r  "bin" ]
  then
    rm -r bin obj
  fi
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
  rm -f report.pdf presentation.pdf 
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
  echo "Usage: bash proyecto.sh <command> [options]"
  echo ""
  echo "proyecto.sh is a bash script that provides commands for build and run Moogle!"
  echo "and generate all the documentation related to it."
  
  echo ""
  echo "List of commands:"
  echo "  build                 - to build Moogle"
  echo "  run                   - to compile and run Moogle, and open in the browser the search page"
  echo "  report                - to generate Moogle's report"
  echo "  report [compiler]     - to generate Moogle's report using 'compiler'"
  echo "  slides                - to generate Moogle's slides"
  echo "  slides [compiler]     - to generate Moogle's slides using 'compiler'"
  echo "  clean                 - to clean up the files generated in the build process"
  echo "  veryclean             - to clean up all documentation and all auxiliary files"
  echo "  show_report [viewer]  - to show Moogle's report using 'viewer'"
  echo "  show_slides [viewer]  - to show Moogle's slides using 'viewer'"
  echo "  all_docs              - to build all Moogle's documentation and clean up all auxiliary files"
  echo "  help                  - to show this information"
  echo ""
  echo ""
  echo "                                                       This script has Super MatCom Powers :)"
}

"$@"