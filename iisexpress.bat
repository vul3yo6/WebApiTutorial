::
:: ±Ò°Ê IIS Express
::
@echo =========  IIS Express  ===================
@echo Enabling IIS Express run web site

cd /D "C:\Program Files\IIS Express"
iisexpress.exe /config:D:\fs\Repos\WebApiTutorial\.vs\config\applicationhost.config /site:WebApiTutorial
pause
