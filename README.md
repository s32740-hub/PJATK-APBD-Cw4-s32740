# LegacyRenewalApp — Dokumentacja refaktoryzacji

## Spis treści

1. [Cel refaktoryzacji](#1-cel-refaktoryzacji)
2. [Problemy w oryginalnym kodzie](#2-problemy-w-oryginalnym-kodzie)
3. [Zastosowane zasady i wzorce](#3-zastosowane-zasady-i-wzorce)

---

## 1. Cel refaktoryzacji

Projekt `LegacyRenewalApp` obsługuje proces odnowienia subskrypcji: oblicza rabaty, opłaty dodatkowe, podatek i generuje fakturę. Aplikacja działała poprawnie, jednak jej kod był trudny w utrzymaniu i rozszerzaniu.

Celem refaktoryzacji było **poprawienie jakości kodu bez zmiany logiki biznesowej** - przy zachowaniu publicznego kontraktu wykorzystywanego przez  `LegacyRenewalAppConsumer`.

---

## 2. Problemy w oryginalnym kodzie

### 2.1 Jedna długa metoda z wieloma odpowiedzialnościami

Oryginalna klasa `SubscriptionRenewalService` posiadała jedną metodę, która jednocześnie:

- walidowała dane wejściowe,
- pobierała dane z repozytoriów,
- obliczała rabaty (przez rozbudowane bloki `if-else`),
- obliczała opłaty dodatkowe (support, płatność),
- obliczała podatek,
- budowała obiekt faktury,
- zapisywała fakturę,
- wysyłała e-mail.

To naruszenie zasady **Single Responsibility Principle (SRP)** powodowało, że każda zmiana w dowolnym obszarze biznesowym wymagała modyfikacji tej samej klasy.

### 2.2 Twarde uzależnienie od statycznej klasy zewnętrznej

Bezpośrednie wywołania `LegacyBillingGateway.SaveInvoice()` i `LegacyBillingGateway.SendEmail()` wewnątrz serwisu tworzyły silne sprzężenie z zewnętrzną, niemodyfikowalną biblioteką. Nie było możliwości zastąpienia tej zależności.

### 2.3 Rozbudowane bloki `if-else` dla typów rabatów, podatków i opłat

Logika w stylu:
```csharp
if (segment == "Gold") discount = 0.10m;
else if (segment == "Platinum") discount = 0.15m;
// itd.
```
Każde dodanie nowego typu rabatu wymagało modyfikacji istniejącej metody - naruszenie **Open/Closed Principle (OCP)**.

### 2.4 Bezpośrednie tworzenie zależności wewnątrz klasy (`new`)

Klasa sama tworzyła wszystkie swoje zależności, uniemożliwiając ich podmianę z zewnątrz (brak Dependency Injection).

---

## 3. Zastosowane zasady i wzorce

### Single Responsibility Principle (SRP)

Każda klasa ma teraz dokładnie jedną odpowiedzialność:

| Klasa | Odpowiedzialność |
|---|---|
| `RenewalValidator` | Walidacja danych wejściowych |
| `BaseCalculation` | Obliczenie kwoty bazowej |
| `DiscountProvider` | Agregacja i zastosowanie rabatów |
| `DiscountValidator` | Walidacja wyniku po rabacie |
| `SupportFeeProvider` | Obliczenie opłaty za wsparcie |
| `PaymentFeeProvider` | Obliczenie opłaty za metodę płatności |
| `TaxProvider` | Wyznaczenie stawki podatkowej |
| `FinalSumCalculator` | Obliczenie sumy końcowej |
| `RenewalInvoiceFactory` | Budowanie obiektu faktury |
| `SaveInvoiceService` | Zapis faktury |
| `LegacyEmailAdapter` | Wysyłka powiadomienia e-mail |
| `RenewalProcessService` | Koordynacja: zapis + notyfikacja |
| `SubscriptionRenewalServiceOrchestrator` | Przepływ całego procesu biznesowego |

### Open/Closed Principle (OCP) - wzorzec Strategy

Zamiast bloków `if-else`, każdy typ rabatu, podatku i opłaty to **osobna klasa implementująca wspólny interfejs**. Dodanie nowego wariantu nie wymaga modyfikacji istniejącego kodu - wystarczy dodać nową klasę.

**Przykład dla rabatów:**
```csharp
public interface IDiscountStrategy
{
    bool IsMatch(DiscountContext context);
    (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context);
}

public class GoldSegmentDiscount : IDiscountStrategy
{
    public bool IsMatch(DiscountContext context) => context.Segment == "Gold";
    public (decimal discount, string note) Calculate(decimal baseAmount, DiscountContext context)
        => (baseAmount * 0.10m, "gold discount; ");
}
```

Ten sam wzorzec zastosowano dla: `ITaxCalculator`, `ISupportFee`, `IPaymentFee`.

### Dependency Inversion Principle (DIP) + Dependency Injection

`SubscriptionRenewalServiceOrchestrator` nie tworzy żadnej zależności samodzielnie, wszystkie są wstrzykiwane przez konstruktor:

```csharp
public SubscriptionRenewalServiceOrchestrator(
    ICustomerPlanRepository customerPlanRepository,
    ISubscriptionPlanRepository subscriptionPlanRepository,
    IRenewalValidator renewalValidator,
    IBaseCalculation baseCalculation,
    IDiscountProvider discountProvider,
    // ...
)
```

### Adapter Pattern - izolacja `LegacyBillingGateway`

Statyczna klasa zewnętrzna jest opakowana we własne adaptery:

```csharp
// Adapter dla zapisu faktury
public class SaveInvoiceService : IBillingRepository
{
    public void Save(RenewalInvoice invoice)
        => LegacyBillingGateway.SaveInvoice(invoice);
}

// Adapter dla wysyłki e-mail
public class LegacyEmailAdapter : INotificationService
{
    public void Notify(RenewalInvoice invoice, string email)
    {
        // ...
        LegacyBillingGateway.SendEmail(email, subject, body);
    }
}
```

Dzięki temu reszta kodu zależy od interfejsów `IBillingRepository` i `INotificationService`, a nie od konkretnej statycznej klasy.

---
## Autor
Imię i Nazwisko: Hanna Krechyk

Nr Indeksu: s32740
