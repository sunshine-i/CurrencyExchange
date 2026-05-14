# CurrencyExchange

**Course:** Network Application Development

**Author(s):** Sunshine Maree Ilagan

**Student ID(s):** 73656

---

## 1. Introduction

This report presents the design and implementation of CurrencyExchange, a distributed desktop application developed as part of the Network Application Development course. The system enables users to manage a virtual multi-currency wallet and simulate currency trading operations using live exchange rate data sourced from the National Bank of Poland (NBP) public API.

## 2. System Architecture

The application follows a three-tier client–server architecture comprising the following components:

- **WCF Service (`CurrencyExchange.Service`)** — A Windows Communication Foundation web service responsible for communicating with the NBP REST API, caching rate data, and performing exchange calculations. All currency conversions are routed through PLN as the base currency, in accordance with the NBP Table A data format.
- **WPF Client (`CurrencyExchange.Client`)** — A Windows Presentation Foundation desktop application implementing the Model–View–ViewModel (MVVM) pattern. The client consumes the WCF service via a generated proxy and interfaces directly with the database layer for user account and balance management.
- **Database Library (`CurrencyExchange.Database`)** — A .NET class library encapsulating all SQL Server data access logic, including user authentication, balance management, and transaction persistence.

## 3. Implemented Features

The following features were implemented and are fully functional:

- **User authentication** — Account registration and login with SHA-256 password hashing.
- **Live exchange rates** — Real-time retrieval and display of NBP Table A mid-market rates.
- **Virtual top-up** — Simulated deposit of funds into a user's account in any supported currency.
- **Currency trading** — Conversion between any two supported currencies, subject to available balance verification prior to execution. Each trade is committed atomically via a SQL Server transaction.
- **Transaction history** — Persistent record of all top-up and exchange operations, including the applied rate expressed in human-readable form (e.g. `1 USD = 3.9142 PLN`).

## 4. Technologies Used

| Component | Technology |
|---|---|
| Programming language | C# / .NET Framework 4.8 |
| Desktop UI | Windows Presentation Foundation (WPF) |
| Web service | Windows Communication Foundation (WCF) |
| Database | Microsoft SQL Server |
| External API | NBP Open Data API (api.nbp.pl) |
| UI pattern | Model–View–ViewModel (MVVM) |

## 5. Conclusion

The CurrencyExchange application demonstrates the integration of a WCF web service with a WPF client in a structured, layered architecture. The separation of concerns between the service, client, and database layers ensures maintainability and testability. The use of atomic database transactions and server-side balance validation ensures data integrity throughout all trading operations.