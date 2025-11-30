CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Stores" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "OwnerName" character varying(100) NOT NULL,
    CONSTRAINT "PK_Stores" PRIMARY KEY ("Id")
);

CREATE TABLE "Transactions" (
    "Id" uuid NOT NULL,
    "Type" integer NOT NULL,
    "OccurrenceDate" date NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "CPF" character varying(11) NOT NULL,
    "CardNumber" character varying(20) NOT NULL,
    "Time" time NOT NULL,
    "StoreId" uuid,
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Transactions_Stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES "Stores" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Transactions_StoreId" ON "Transactions" ("StoreId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251025154359_InitialCreate', '8.0.21');

COMMIT;