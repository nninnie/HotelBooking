SELECT * 
FROM Guests;

SELECT * 
FROM Rooms
ORDER BY RoomNumber;


SELECT * 
FROM Bookings
WHERE IsDeleted = 0;


SELECT * 
FROM Rooms
WHERE IsOutOfService = 0;


SELECT * 
FROM Guests
WHERE LastName LIKE 'S%';


