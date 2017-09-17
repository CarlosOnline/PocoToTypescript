@if "%1" EQU "" echo Missing checkin comments && goto :EOF
git status
git add -A
git commit -m "%*"
git status
git pull origin master
git push origin master
