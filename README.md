# WindAPIServer
This a socket server of Wind API which return JSON result

How to use

Complie this project and then copy the exe file to the machine which have setuped the Wind Client.

Run it. And then you can send the command below by socket client.

```
edb|M0024135,M5206729,M0024136|ED-1Y|2020-06-16| via socket
```

```
wsd|NH0100.NHF|pre_close,open,high,low,close,volume,amt,dealnum,chg,pct_chg|2020-06-16|2020-06-16|
```

Note: It depends on "WAPIWrapperCSharp.dll" which you can download it from the API document of Wind Client.
