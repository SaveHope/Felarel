# Felarel

Дискорд-бот тайной империи Звездного

# Конфигурация

Бот принимает конфигурацию из четырех источников в следующем порядке приоритета:
1. Параметры запуска командной строки.
2. Переменные окружения.
3. YAML-файл конфигурации выбираемого окружения `Config/appsettings.{DOTNET_ENVIRONMENT}.yaml`.
4. YAML-файл базовой конфигурации `Config/appsettings.yaml`.

Прочие файлы:
- `Config/discord.token хранит в первой строке используемый токен авторизации. В остальных строках могут быть произвольные данные.

Для выбора YAML-файла под конкретное окружение, необходимо задать env-переменную `DOTNET_ENVIRONMENT`, например:
```sh
export DOTNET_ENVIRONMENT=prod
dotnet FelarelDiscord.dll
```

## Способы указания конфигурации

### Аргументы командной строки
Принимаются флаги начинающиеся на двойной дефис `--`. Вложенность подразделяется двоеточиями `:`, с ключом параметра на конце.

Пример:
```sh
dotnet FelarelDiscord.dll --Client:GatewayIntents=Guild
```

### Переменные окружения
Вложенность подразделяется двойными нижними подчеркиваниями `__` с ключом параметра на конце.

Пример:
```env
Client__GatewayIntents=Guild
```
### YAML-файл
Вложенные YAML-объектами секции с ключами параметров.

Пример:
```yaml
Client:
  GatewayIntents: Guild
```

## Описание параметров конфигурации

### Секция Logging

См. NuGet пакеты [`Microsoft.Extensions.Logging`](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview?tabs=command-line), [`Serilog.Extensions.Logging.File`](https://github.com/serilog/serilog-extensions-logging-file).

### Секция Client

[См. документацию Discord.NET (DiscordSocketConfig).](https://docs.discordnet.dev/api/Discord.WebSocket.DiscordSocketConfig.html)

### Секция Interaction

[См. документацию Discord.NET (InteractionServiceConfig).](https://docs.discordnet.dev/api/Discord.Interactions.InteractionServiceConfig.html)

- `DebugGuildId` - По умолчанию `0`. Если здесь указан ID сервера, то бот обновит список команд только для него, а не глобально. Это полезно для быстрой отладки, т.к. глобальное изменение команд занимает долгое время.