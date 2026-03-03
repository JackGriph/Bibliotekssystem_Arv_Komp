namespace Bibliotekssystem_Arv_Komp.Web.Dtos;

// ===== Book DTOs =====
public record BookDto(
    int Id,
    string ISBN,
    string Title,
    string Author,
    int PublishedYear,
    int Pages,
    bool IsAvailable
);

public record BookDetailDto(
    int Id,
    string ISBN,
    string Title,
    string Author,
    int PublishedYear,
    int Pages,
    bool IsAvailable,
    List<LoanDto> Loans
);

public record CreateBookRequest(
    string ISBN,
    string Title,
    string Author,
    int PublishedYear,
    int Pages
);

// ===== Member DTOs =====
public record MemberDto(
    int Id,
    string MemberId,
    string Name,
    string Email,
    DateTime MemberSince,
    int ActiveLoans
);

public record MemberDetailDto(
    int Id,
    string MemberId,
    string Name,
    string Email,
    DateTime MemberSince,
    List<LoanDto> Loans
);

public record CreateMemberRequest(
    string MemberId,
    string Name,
    string Email
);

// ===== Loan DTOs =====
public record LoanDto(
    int Id,
    string BookTitle,
    int BookId,
    string MemberName,
    int MemberId,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate
);

public record CreateLoanRequest(
    int BookId,
    int MemberId
);

// ===== Stats DTO =====
public record StatsDto(
    int BookCount,
    int MemberCount,
    int ActiveLoanCount,
    int OverdueLoanCount
);
