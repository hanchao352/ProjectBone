set WORKSPACE=..\..\

set GEN_CLIENT=%WORKSPACE%\Tools\CfgTools\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\Config

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir ..\..\Project\Assets\GenCodeCfg ^
 --output_data_dir ..\..\Project\Assets\GenJsonData ^
 --gen_types code_cs_unity_json,data_json ^
 -s all 

pause