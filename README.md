# TextProcessor

- Подсчет слов (4-20 символов), которые встречаются не менее 3 раз в текстовых файлах.<br>
- запись в базу с обновлением статистики. К старым значениям прибавляются результаты обработки новых файлов.<br>
- Поддерживается запись из нескольких экземпляров приложения, а также запуск параллельных обработок файлов.<br>
Обработка конфликтов параллелизма производится согласно статье https://learn.microsoft.com/ru-ru/ef/core/saving/concurrency?tabs=data-annotations <br>

База (ms SQL) создается автоматически, <b>НО предварительно, необходимо указать строку подключения к нужному серверу в файле appSettings.json (вставить в пустые кавычки)</b> <br>
![image](https://github.com/user-attachments/assets/f3422b47-498c-4cac-8325-0716c47badac)<br>
Cтроку подключения можно узнать в свойствах сервера.<br>
примерный пример строки: <b>"Data Source=RYZEN-PC\\SQLEXPRESS;Initial Catalog=TextProcessorDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"</b><br>
- где RYZEN-PC\\SQLEXPRESS - ваш сервер<br>
- добавить: Initial Catalog=TextProcessorDb;    (TextProcessorDb - любое название для базы, которая будет создана)<br><br>

Подсчет статистики по файлу идет построчным ограниченным распараллеливанием с записью в конкурентный словарь, затем данные из словаря фильтруются по необходимым критериям и происходит попытка записи в базу одной транзакцией. Если какие-то данные изменились другими файлами, то запрашиваются обновления актуальных данных и новые попытки записи.
![image](https://github.com/user-attachments/assets/1001ca86-49f7-4104-a2de-9c5519af46b2)
