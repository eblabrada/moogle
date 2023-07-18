LATEXCMD = pdflatex  -synctex=1 -interaction=nonstopmode -file-line-error -recorder -output-directory Report/

.PHONY: help
help:
	@echo "This makefile builds Moogle and its documentation"
	@echo ""
	@echo "Available commands are:"
	@echo " make build       - to build Moogle"
	@echo " make dev         - to run Moogle"
	@echo " make docs        - to build Moogle's documentation"
	@echo " make latexclean  - to clean up the latex's build process"
	@echo " make veryclean   - to clean up all documentation"
	@echo " make help        - to show this information"
	@echo ""

.PHONY: build
build:
	dotnet build

.PHONY: dev
dev:
	dotnet watch run --project MoogleServer

latexbuild: 
	@cd Report
	$(LATEXCMD) report.tex </dev/null
	$(LATEXCMD) presentation.tex </dev/null
	@cd ..
	@cp Report/report.pdf report.pdf
	@cp Report/presentation.pdf presentation.pdf

.PHONY: latexclean
latexclean:
	@cd Report && rm -f report.aux report.fls report.log report.fdb_latexmk report.out report.synctex.gz report.pdf
	@cd Report && rm -f presentation.aux presentation.fls presentation.log presentation.nav presentation.synctex.gz presentation.snm presentation.toc presentation.fdb_latexmk presentation.pdf
	
.PHONY: veryclean
veryclean: | latexclean
	@rm -f report.pdf presentation.pdf 
	
.PHONY: docs
docs: latexbuild | latexclean