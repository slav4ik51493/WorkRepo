---
paths:
  - "*.cs"
---

# Code Style Rules — C#

---

## 1. Общие правила

- **ВСЕГДА** прогоняй компиляцию и все тесты после завершения задачи.
- При массовом падении тестов (более 1%) — немедленно исправляй, это наивысший приоритет.
- При локальном падении (1–3 теста) — исправляй в рамках текущей задачи.
- **НИКОГДА** не удаляй инфраструктурный код (методы расширения, классы) даже при отсутствии ссылок.
- Крупный рефакторинг (2+ DLL или 100+ файлов) выноси в отдельную ветку с пошаговыми PR.

---

## 2. Отступы, переносы строк и разделение членов

### Переносы строк

- **ВСЕГДА** разделяй все члены типа (поля, свойства, конструкторы, методы) пустой строкой.
- **ВСЕГДА** разделяй логические шаги внутри метода пустыми строками.
- **ИСКЛЮЧЕНИЕ:** инициализация объекта через несколько строк (`this.X = ...`) — без пустых строк.

```csharp
// ПРАВИЛЬНО
public override async Task<int> GetSubscribersCountAsync()
{
    var token = await this.GetTokenFromStorageAsync();

    if (token.IsExpired)
    {
        token = await this.RefreshLongLivedTokenAsync(token);

        await this.SaveTokenToStorageAsync(token);
    }

    return await this.FetchFollowersCountAsync(token);
}
```

### Отступы

- **ВСЕГДА** используй ENTER + TAB для каждого уровня вложенности.
- **НИКОГДА** не выравнивай пробелами.
- **ИСКЛЮЧЕНИЯ** (описаны ниже): `throw new`, составное выражение, составное булевое выражение.

### Тернарный оператор
- **ВСЕГДА** Условие — с новой строки, один TAB от уровня присваивания.
- **ВСЕГДА**`?` и `:` — каждый на новой строке, один дополнительный TAB от условия.

```csharp
this.rbody.linearDamping =
    this.AirBrakes
        ? (this.originalDrag + extraDrag) * this.airBrakesEffect
        : this.originalDrag + extraDrag;
```

### Объявление переменных

- **ВСЕГДА** используй `var`, где тип выводится из правой части.
- **НИКОГДА** не объявляй несколько переменных через запятую в одном выражении.

```csharp
// ПРАВИЛЬНО
var a = 0;
var b = 0;
var c = 0;
```

### Инициализация объектов

- Без параметров — допустима одна строка.
- С параметрами — переносить, каждый аргумент на отдельной строке.

```csharp
// ПРАВИЛЬНО: нет параметров
var token = new LongLivedToken();

// ПРАВИЛЬНО: есть параметры
var request =
    new HttpRequestMessage(
        HttpMethod.Get,
        url);
```

### Составное выражение

- Оператор (`+`, `-`, `*`, `/` и т.д.) — **ВСЕГДА** в начале новой строки.
- **ВСЕГДА** оператор смещён на **2 пробела левее** колонки операнда, чтобы все операнды выровнялись по одной вертикальной позиции.
- Правило применяется **на каждом уровне вложенности независимо**: у каждого уровня свой столбец операндов и свой столбец операторов (на 2 левее).

```
колонка оператора = колонка операнда − 2 пробела
```

```csharp
// ПРАВИЛЬНО: строки — оператор на 2 пробела левее первого операнда (col 7 → col 5)
private static string BuildUrl(Settings settings)
    => "https://graph.instagram.com/access_token"
     + "?grant_type=ig_exchange_token"
     + $"&client_secret={settings.ClientSecret}"
     + $"&access_token={settings.AccessToken}";

// ПРАВИЛЬНО: арифметика — первый операнд на col 4, операторы на col 2
var score =
    successRate
  * parameters.Uptime
  * inversePing
  * speedFactor
  * parameters.Weight
  - parameters.PingMs / 100.0;

// ПРАВИЛЬНО: вложенное выражение — внешний уровень col 4 / операторы col 2,
//            внутренний уровень (внутри вызова) col 8 / операторы col 6
var result =
    Math.Exp(
        -Math.Pow(x - mu, 2)
      / (2 * Math.Pow(sigma, 2)))
  / (sigma * Math.Sqrt(2 * Math.PI))
  + (offset * logicalScale)
  - (correctionFactor / smoothingValue);
```

### LINQ

- Каждый метод цепочки — с новой строки.

```csharp
public IEnumerable<Account> GetActiveVerifiedAccounts(IEnumerable<Account> accounts)
    => accounts
        .Where(account => account.IsActive)
        .Where(account => account.IsVerified)
        .OrderByDescending(account => account.CreatedAt)
        .Select(account => account);
```

### Составное булевое выражение

- `||` и `&&` — в начале строки, один TAB от уровня оператора.

```csharp
if (string.IsNullOrEmpty(str1)
 || string.IsNullOrEmpty(str2))
{
    ...
}
```

### Throw

- **ВСЕГДА** добавляй перенос строки перед `throw`.
- **ВСЕГДА** `new` при выбросе нового исключения — на следующей строке с одним TAB (вне зависимости от вложенности).
- **ВСЕГДА** последний символ `w` от `throw` и `new` должен быть на одной и той же позиции в строке, как это указано в примере ниже.
 
```csharp
throw
  new InvalidOperationException("Сообщение об ошибке.");
```

### Expression-body (`=>`)

- `=>` — всегда на новой строке (ENTER + TAB).

```csharp
public bool IsExpired
    => DateTime.UtcNow.AddDays(2) > this.AccessTokenUpdatedAt.AddSeconds(this.ExpiresInSeconds);
```

### Return

- **ВСЕГДА** отделяй `return` пустой строкой от предшествующего кода.

```csharp
var result = this.Compute();

return result;
```

---

## 3. Именование

- **НИКОГДА** не используй сокращения: `res`, `usr`, `msg`, `btn`, `cnt`, `cfg`, `ctx`, `repo`, `svc` — только полные слова.
- **НИКОГДА** не используй бессмысленные имена: `data`, `info`, `value`, `item`, `obj`, `temp`.
- Если в классе одна фабрика — называй `factory`, не `httpClientFactory`. Не уточняй без необходимости.
- Булевые переменные и свойства — **ВСЕГДА** с префиксом `is`, `has`, `can`, `should`.
- Классы — **ВСЕГДА** существительные: `TokenManager`, `AccountRepository`, `PaymentService`.
- Методы — **ВСЕГДА** начинаются с глагола: `GetSubscribersAsync`, `SaveTokenAsync`, `BuildUrl`.

```csharp
// ПРАВИЛЬНО
var isExpired = token.ExpiresAt < DateTime.UtcNow;
var hasAdminAccess = user.Role == Role.Admin;
var response = await this.GetAsync(id);
var tokenLifetimeSeconds = token.ExpiresInSeconds;
```

---

## 4. Структура проекта

- Все абстрактные классы и интерфейсы — **ТОЛЬКО** в папке `Abstractions/`.
- Все базовые классы — **ТОЛЬКО** в папке `Base/`.

```
Providers/
├── Abstractions/
│   ├── ISubscribersCountProvider.cs
│   └── ITokenProvider.cs
├── Base/
│   └── SubscribersCountHttpProvider.cs
├── Instagram/
│   ├── Models/
│   └── InstagramProvider.cs
└── TikTok/
    ├── Models/
    └── TikTokProvider.cs
```

---

## 5. Пространства имён (using)

Сортируй блоки строго в следующем порядке:

1. Системные (`System.*`)
2. Microsoft (`Microsoft.*`)
3. Сторонние библиотеки (3rd party)
4. Внутренние библиотеки (submodules, dll)
5. Текущий проект
6. Алиасы (псевдонимы namespace — с маленькой буквы, класса — с большой)
7. Статичные `using static` (избегай)

- Внутри каждого блока — **ВСЕГДА** алфавитный порядок.
- **ИЗБЕГАЙ** статичных `using static`.

---

## 6. Классы

### Общее

- Каждый класс — **РОВНО** одна ответственность. При нарушении — разбивай.
- При дублировании свойств или логики — **ВСЕГДА** выноси в базовый класс или интерфейс.
- Если класс не имеет наследников — **ВСЕГДА** помечай `sealed`.

### Порядок блоков внутри класса

1. Поля
2. Свойства
3. Конструкторы
4. Методы
5. Вложенные классы

### Конструкторы

- Сортируй по убыванию числа параметров.
- При 2+ параметрах — каждый на отдельной строке.
- Допускаются primary-конструкторы (C# 12+).

### Порядок методов

Сортируй по:
1. Модификатор: `protected` → `private` → `public`
2. Перегрузка: `override` → `abstract` → `virtual` → обычные
3. Число параметров (по убыванию)

### ASP.NET контроллеры и сервисы/репозитории

Порядок методов сверху вниз:
1. Export (возвращает файлы)
2. GetMany → GetAll → Get
3. Create
4. Update
5. Delete

### partial классы

- При разрастании класса допускай `partial`. Имя файла — `ClassName.PartName.cs`.

---

## 7. Документация (комментарии)

- Описание метода **НЕ** содержит слово «метод», класса — «класс».
- Комментарии в тегах пиши в одну строку, заканчивай точкой.
- `<param/>`, `<returns/>`, `<exception/>` — **НИКОГДА** не оставляй пустыми.
- При реализации интерфейса — используй `<inheritdoc/>`.
- Если реализация имеет особенности — пиши собственный комментарий вместо `<inheritdoc/>`.
- Внутри методов комментируй **ТОЛЬКО** нетривиальные вычисления с неочевидными константами.

```csharp
/// <summary>Получает подписчиков по идентификатору аккаунта.</summary>
/// <param name="accountId">Идентификатор аккаунта.</param>
/// <returns>Список подписчиков.</returns>
/// <exception cref="InvalidOperationException">Если аккаунт не найден.</exception>
public Task<List<Subscriber>> GetSubscribersAsync(Guid accountId)
```

---

## 8. Атрибуты

- **КАЖДЫЙ** атрибут — в отдельных квадратных скобках на отдельной строке.
- **НИКОГДА** не объединяй несколько атрибутов в одних скобках `[HttpGet, Authorize]`.
- **НИКОГДА** не ставь атрибуты на одной строке с объявлением.

```csharp
// ПРАВИЛЬНО
[HttpGet]
[Authorize]
[ProducesResponseType(StatusCodes.Status200OK)]
public Task<IActionResult> GetAsync()
```

---

## 9. Явные квалификаторы `this` и `base`

- **ВСЕГДА** используй `this.` при обращении к членам текущего класса.
- **ВСЕГДА** используй `base.` при обращении к членам родительского класса.
- **НИКОГДА** не обращайся к членам класса без квалификатора.

---

## 10. Ключевые слова

### catch

- Имя переменной исключения — **ВСЕГДА** `ex`.
- **НИКОГДА** не перехватывай голый `Exception` без необходимости — сужай тип.
- **НИКОГДА** не оставляй пустой `catch`-блок.
- **ВСЕГДА** логируй детали исключения.
- При перехвате только для логирования — используй `throw;` для сохранения stack trace.
- При оборачивании — **ВСЕГДА** передавай оригинальное исключение как `innerException`.

```csharp
// ПРАВИЛЬНО
catch (HttpRequestException ex)
{
    this.logger.LogError(ex, "Ошибка при получении данных от Instagram API.");

    throw;
}
```

### if

- **ВСЕГДА** используй фигурные скобки, даже для однострочного тела.
- **НИКОГДА** не пиши `if` без `{}`.

### Regex

- **ВСЕГДА** используй `[GeneratedRegex]` — не инлайн `new Regex(...)`.

### using

- **ВСЕГДА** используй фигурные скобки для `using`.
- **ИСКЛЮЧЕНИЕ:** несколько последовательных `using` подряд — допустима форма `using var`.

---

## 11. Методы

- Каждый метод делает **РОВНО** одну вещь. Если название содержит «и» — разбивай.
- Размер метода — не более 20–30 строк (один экран).
- При 3+ параметрах — каждый на отдельной строке и при объявлении, и при вызове.

```csharp
// ПРАВИЛЬНО: объявление
protected async Task<string> GetJsonResponseAsync(
    string request,
    AuthenticationHeaderValue authentication,
    CancellationToken cancellationToken)

// ПРАВИЛЬНО: вызов
var response =
    await this.GetJsonResponseAsync(
        requestUrl,
        authHeader,
        cancellationToken);
```

---

## 12. Константы

- **ВСЕГДА** выноси магические строки, числа и конфигурационные ключи в `Constants.cs`.
- **НИКОГДА** не дублируй статичные значения по всему проекту.
- Структурируй `Constants.cs` по вложенным статичным классам по домену.

```csharp
internal static class Constants
{
    internal static class Instagram
    {
        internal const string TokenSecretKey = "instagram-token";

        internal const string BaseUrl = "https://graph.instagram.com";

        internal const int TokenExpirationBufferDays = 2;
    }
}
```

---

## 13. Сравнение строк

- **ВСЕГДА** используй `string.Equals(a, b, StringComparison.OrdinalIgnoreCase)`.
- **НИКОГДА** не используй `a == b` для строк.
- **НИКОГДА** не используй `a.Equals(b, ...)` — только статичный `string.Equals`.