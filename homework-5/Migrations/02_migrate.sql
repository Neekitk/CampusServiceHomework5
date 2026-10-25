UPDATE "CertificateRequests"
SET "StatusCode" =
    CASE
        WHEN "Status" = 'Created' THEN 1
        WHEN "Status" = 'InProgress' THEN 2
        WHEN "Status" = 'Completed' THEN 3
        WHEN "Status" = 'Rejected' THEN 4
        ELSE 0
    END
WHERE "StatusCode" IS NULL;