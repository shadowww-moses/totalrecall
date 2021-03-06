

# Entity

Entity - это понятие ближе к сущности, типу. Поэтому обычно используется в разных конфигурационных методах:

<img src="img\image-20200509090932633.png" alt="image-20200509090932633" style="zoom:80%;" />

# Entry

Entry - это ближе к экземпляру сущности. Поэтому, используя Entry, можно по экземпляру через контекст добраться до Shadow-свойств этой сущности, а также, например, контролировать состояние каждого экземпляра вручную:

<img src="img\image-20200509084231112.png" alt="image-20200509084231112" style="zoom:80%;" />

В этом примере показано как управлять состоянием отдельного экземпляра сущности вручную. В данном случае это нужно, потому что из-за disconnected-сценария мы должны как-то самостоятельно сообщить контексту, что именно мы изменили. 

Если выполнить `contextPost.Playlists.Update(playlist)` то контекст обновит все плейлисты пользователя и даже самого пользователя. Поэтому мы явно указываем для отдельно взятой цитаты состояние Modified, чтобы избежать лишних операций обновления.
