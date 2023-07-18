#!/bin/bash

compileReport() {
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Report/ Report/report.tex </dev/null
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Report/ Report/report.tex </dev/null
}

compilePresentation() {
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Report/ Report/presentation.tex </dev/null
  pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -shell-escape -output-directory Report/ Report/presentation.tex </dev/null
}

latexclean() {
  cd Report
  rm -f report.aux report.fls report.log report.fdb_latexmk report.out report.synctex.gz report.pdf
  rm -f presentation.aux presentation.fls presentation.log presentation.nav presentation.synctex.gz presentation.snm presentation.toc presentation.fdb_latexmk presentation.pdf
  cd ..
}

latexbuild() {
  compileReport;
  compilePresentation;
	cp Report/report.pdf report.pdf
	cp Report/presentation.pdf presentation.pdf
  latexclean;
}

veryclean() {
	latexclean;
  rm -f report.pdf presentation.pdf 
}

menu() {
  echo "This script builds Moogle and its documentation"
  echo ""
  echo "build       - to build Moogle"
  echo "dev         - to run Moogle"
  echo "docs        - to build Moogle's documentation"
  echo "clean       - to clean up the latex's build process"
  echo "veryclean   - to clean up all documentation"
  echo "help        - to show this information"
  echo "exit        - to close the script"
  echo ""
}

menu;

while :
do
  read command
  if [ $command = build ]
    then dotnet build
  elif [ $command = dev ]
    then dotnet watch run --project MoogleServer
  elif [ $command = docs ]
    then latexbuild
  elif [ $command = clean ]
    then latexclean;
  elif [ $command = veryclean ]
    then veryclean;
  elif [ $command = exit ]
    then break  
  elif [ $command = help ]
    then menu;
  else
    then:
      echo "" 
      echo "Invalid command!"
      menu;
  fi
  
  echo ""
done