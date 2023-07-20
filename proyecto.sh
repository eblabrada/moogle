#!/bin/bash

compileReport() {
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Informe/ Informe/report.tex </dev/null
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Informe/ Informe/report.tex </dev/null
}

compilePresentation() {
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Presentacion/ Presentacion/presentation.tex </dev/null
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Presentacion/ Presentacion/presentation.tex </dev/null
}

latexclean() {
  cd Informe
  rm -f report.aux report.fls report.log report.fdb_latexmk report.out report.synctex.gz
  cd ..
  cd Presentacion
  rm -f presentation.aux presentation.fls presentation.log presentation.nav presentation.synctex.gz presentation.snm presentation.toc presentation.fdb_latexmk
  cd ..
}

latexbuild() {
  compileReport;
  compilePresentation;
	cp Informe/report.pdf report.pdf
	cp Presentacion/presentation.pdf presentation.pdf
  latexclean;
}

veryclean() {
	latexclean;
  rm -f report.pdf presentation.pdf 
}

showReport() {
  if [ ! -f  "Informe/report.pdf" ]
    then compileReport;
  fi  
  xdg-open Informe/report.pdf
}

showPresentation() {
  if [ ! -f  "Presentacion/presentation.pdf" ]
    then compilePresentation;
  fi
  xdg-open Presentacion/presentation.pdf
}

menu() {
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
  echo "docs        - to build all Moogle's documentation and clean up all unnecessary files"
  echo "help        - to show this information"
  echo ""
}

if [ $1 = build ]
  then dotnet build
elif [ $1 = run ]
  then dotnet watch run --project MoogleServer
elif [ $1 = report ]
  then compileReport;
elif [ $1 = slides ]
  then compilePresentation;
elif [ $1 = clean ]
  then latexclean;
elif [ $1 = veryclean ]
  then veryclean;
elif [ $1 = docs]:
  then latexbuild;
elif [ $1 = help ]
  then menu;
elif [ $1 = show_report ]
  then showReport;
elif [ $1 = show_slides ]
  then showPresentation;
elif [ $1 = docs ]
  then latexbuild;
else
  then:
    echo "Invalid command!"
    menu;
fi