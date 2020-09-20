@echo off
wget https://gist.githubusercontent.com/dashed/6714393/raw/ae966d9d0806eb1e24462d88082a0264438adc50/github-pandoc.css
echo ^<style^> > a.txt
echo ^</style^> > b.txt
type a.txt github-pandoc.css b.txt > pandoc.css
pandoc -s --toc README.md -H pandoc.css -o README.html
del github-pandoc.css a.txt b.txt pandoc.css
