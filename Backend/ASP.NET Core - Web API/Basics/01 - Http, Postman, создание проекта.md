

# Что это?

Мануал начального уровня по созданию простого Web API на ASP.NET Core 3.1. О многих вещах здесь может быть рассказано поверхностно и не полностью. Поэтому, если написано, что А делает B и C, вполне возможно, что оно еще делает и D, и E, и F и еще кучу вещей, о которых я на момент написания этого мануала еще даже не знаю.

О красивом коде и best practices речь не идет. Здесь только скелет технологии с простейшими примерами, не отвлекающими от сути.

При расширении знаний будут составляться новые мануалы с более высокой отметкой сложности. Этот будет оставаться простым.

Мануал составляется из рассчета, что я буду помнить базовые вещи, например такие, как создать проект, посмотреть свойства сборки, что контроллеры надо класть в папку Controllers и прочее.

Мануал следует просмотреть целиком, а не стараться сразу писать, потому что это обзорная напоминалка, а не пошаговый учебник с нуля. Последовательность изложения такая, что полная картина складывается только в конце.





# Кратко об HTTP

Структура сообщений, которыми обмениваются по протоколу HTTP, состоит из трех частей:

![image-20200423084216107](img/image-20200423084216107.png)

<p align="center">[1]</p>

| Глагол | Намерение, суть действия                      |
| ------ | --------------------------------------------- |
| GET    | Получить ресурс                               |
| POST   | Добавить добавить                             |
| PUT    | Обновить ресурс                               |
| DELETE | Удалить ресурс                                |
| PATCH  | Обновить ресурс частично (используется редко) |

<p align="center">[2]</p>

Заголовки — разная служебная информация. Например, тип контента, дата и время запроса, тип сервера. На данный момент не сталкивался плотно с заголовками, поэтому не могу сказать ничего действительно ценного о них.

Коды статусов описаны дальше, в разделе про действия контроллеров.

<p align="center">[3]</p>

Контент — очевидно, данные, передающиеся вместе с запросом.



## Ресурсы, URI, Query String

**Ресурсами** называется все, к чему дает доступ Web API. Чем бы они ни были — число, строка, простой объект, сложный объект, что-то еще — все это ресурсы.

**URI** – Uniform Resource Identifier – уникальный идентификатор ресурса. Я не понимаю, чем он отличается от URL и не вижу сейчас смысла понимать это. Я понимаю это просто как ссылку.

**Query String** – часть запроса, которая содержит параметры, использующиеся, например, для фильтрации данных. Идет после вопросительного знака:

![image-20200423085720650](img/image-20200423085720650.png)





# POSTMAN

## Настройки

Здесь всего две настройки. Первая — чтобы при возникновении ошибок нас не перекидывало на другие страницы. А вторая просто для визуального удобства.

![image-20200423091750814](img/image-20200423091750814.png)





# Проект

## Создание проекта

Проект создавался по шаблону Web API, со снятой галочкой конфигурирования для HTTPS. При таком создании в проекте будет только папка Controllers с одним контроллером погоды и файл Startup.cs



## Настройка проекта

Есть две полезные настройки Web API-проекта:

- Чтобы проект запускался всегда на одном и том же порте
- Чтобы при старте проекта не запускался браузер (отлаживать API будем через программу Postman)

ПКМ по проекту > Properties, вкладка Debug

![image-20200423090033266](img/image-20200423090033266.png)
