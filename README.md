# XRPLMarketMaker
An XRPL liquidity bot

## Features

- Get the full order book of a provided currency code and issuer address
- Simulate liquidity based on settings
- Run market maker bot based on settings

## Requirements

- [Visual Studio 2019 or greater](https://visualstudio.microsoft.com/downloads/)

## Settings

- **WebSocketURL**: Main Net: 	wss://s1.ripple.com/  wss://xrplcluster.com/  Test Net: wss://s.altnet.rippletest.net/
- **BotAddress**: Address that holds the liquidity tokens
- **BotSecret**: Secret to the liquidity address. KEEP THIS PRIVATE AND SAFE!
- **CurrencyCode**: Currency code of EMBRS token (can be any token)
- **IssuerAddress**: //Address that issued the token above
- **TransferFee**: Usually not applicable. Leave at 0 if unsure. TransferRate of your token in %, must be between 0 and 100
- **BotTokenAmt**: Amount to send in each rewards txn
- **AccountLinesThrottle**: Number of seconds between request calls. Recommended not to change. Lower settings could result in a block from web service hosts
- **TxnThrottle**: Number of seconds between request calls. Recommended not to change. Lower settings could result in a block from web service hosts.
- **FeeMultiplier**: How many times above average fees to pay for reliable transactions
- **MaximumFee**: Maximum number of drops willing to pay for each transaction
- **Steps**: Number of ask and bid offers around the mid price
- **Interval**: Multiplier of price increase/decrease around the mid price per step

## Getting Started

- Ensure that the config/settings.json file is completely filled out
- Run the project in Visual Studio 2019
- Option 1 will pull the complete order book of Currency_Code and show all asks, bids, mid price, and spread
- Option 2 shows the same information as Option 1 but will simulate the bot's offers based on BotTokenAmt, Steps and Interval
- Option 3 run bot until any key is pressed updating offers every hour based on current market data
- Option 4 will exit
