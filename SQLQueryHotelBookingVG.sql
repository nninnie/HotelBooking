SELECT 
    b.Id AS BookingId,
    g.FirstName,
    g.LastName,
    r.RoomNumber,
    b.FromDate,
    b.ToDate
FROM Bookings b
JOIN Guests g ON b.GuestId = g.Id
JOIN Rooms r ON b.RoomId = r.Id
WHERE b.IsDeleted = 0;


SELECT 
    i.Id AS InvoiceId,
    g.FirstName,
    g.LastName,
    i.TotalAmount,
    i.IsPaid
FROM Invoices i
JOIN Guests g ON i.GuestId = g.Id;


SELECT 
    p.Id AS PaymentId,
    g.FirstName,
    g.LastName,
    p.Amount,
    p.PaymentDate,
    p.IsSuccessful
FROM Payments p
JOIN Invoices i ON p.InvoiceId = i.Id
JOIN Guests g ON i.GuestId = g.Id;


SELECT 
    g.FirstName,
    g.LastName,
    COUNT(b.Id) AS NumberOfBookings
FROM Guests g
LEFT JOIN Bookings b ON g.Id = b.GuestId
GROUP BY g.FirstName, g.LastName;



SELECT 
    r.RoomNumber,
    COUNT(b.Id) AS TotalBookings
FROM Rooms r
LEFT JOIN Bookings b ON r.Id = b.RoomId
GROUP BY r.RoomNumber;



SELECT * 
FROM Rooms r
WHERE r.Id NOT IN (
    SELECT RoomId
    FROM Bookings
    WHERE IsDeleted = 0
      AND '2026-05-10' BETWEEN FromDate AND ToDate
);


SELECT 
    g.FirstName,
    g.LastName
FROM Guests g
WHERE (
    SELECT COUNT(*) 
    FROM Bookings b 
    WHERE b.GuestId = g.Id AND b.IsDeleted = 0
) > (
    SELECT AVG(BookingCount)
    FROM (
        SELECT COUNT(*) AS BookingCount
        FROM Bookings
        WHERE IsDeleted = 0
        GROUP BY GuestId
    ) AS x
);