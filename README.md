# NameGuard

CounterStrikeSharp-плагин для CS2, который блокирует подмену ника и клан-тэга игрока (в том числе читами, меняющими их автоматически).

![Total Downloads](https://img.shields.io/github/downloads/patthsone/NameGuard/total?style=flat&label=Total%20Downloads&labelColor=rgba(0%2C%2070%2C%20114%2C%201)&color=rgba(255%2C%20255%2C%20255%2C%201)) 
![Latest Release](https://img.shields.io/github/v/release/patthsone/NameGuard?style=flat&label=Latest%20Release&labelColor=rgba(0%2C%2070%2C%20114%2C%201)&color=rgba(255%2C%20255%2C%20255%2C%201)) 

[Discord сервер](https://discord.gg/VmJzFBD6wf)

**Автор:** PattHs

## Как это работает

- При полном подключении игрока (`EventPlayerConnectFull`) плагин запоминает его текущий ник и клан-тэг как эталонные.
- При смене ника через игровое событие (`EventPlayerChangename`, команда `name`) плагин мгновенно возвращает оригинальный ник.
- Раз в `PollIntervalSeconds` секунд (по умолчанию 1 секунда) плагин проверяет всех подключённых игроков и сравнивает их текущий ник/тэг с эталонными — это перехватывает читы, которые меняют ник или тэг напрямую, в обход игрового события.
- Все возвраты происходят без кика и без сообщений в чат.

Эталонные значения сбрасываются при отключении игрока и заново фиксируются при следующем подключении.

## Требования

- CounterStrikeSharp (CS2 сервер)
- .NET SDK 10.0 для сборки

## Сборка

```
compile.bat
```

Скрипт публикует проект и собирает готовую структуру в папке `compile\`:

```
compile\addons\counterstrikesharp\plugins\NameGuard\
compile\addons\counterstrikesharp\configs\plugins\NameGuard\
```

## Установка

Скопируйте содержимое папки `compile\addons\` в папку `addons\` на сервере CS2 (`csgo\addons\`).

После первого запуска плагин создаст конфиг:

```
addons\counterstrikesharp\configs\plugins\NameGuard\NameGuard.json
```

## Конфигурация

| Параметр | По умолчанию | Описание |
|---|---|---|
| `RevertName` | `true` | Возвращать оригинальный ник при его смене |
| `RevertClanTag` | `true` | Возвращать оригинальный клан-тэг при его смене |
| `PollIntervalSeconds` | `1.0` | Интервал проверки игроков на подмену ника/тэга, в секундах |
