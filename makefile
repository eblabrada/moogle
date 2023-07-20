LATEXCMDS = pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -output-directory Presentacion/
LATEXCMDI = pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -output-directory Informe/

.PHONY: help
help:
	@echo "This makefile builds Moogle and its documentation"
	@echo ""
	@echo "Available commands are:"
	@echo " make build       - to build Moogle"
	@echo " make dev         - to run Moogle"
	@echo " make help        - to show this information"
	@echo ""

.PHONY: build
build:
	dotnet build

.PHONY: dev
dev:
	dotnet watch run --project MoogleServer
