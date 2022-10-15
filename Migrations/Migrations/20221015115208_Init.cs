using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Migrations.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abonnement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    validity = table.Column<byte>(type: "smallint", nullable: false),
                    number_of_visits = table.Column<byte>(type: "smallint", nullable: false),
                    base_price = table.Column<float>(type: "real", nullable: false),
                    discount = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abonnement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_trainer = table.Column<bool>(type: "boolean", nullable: false),
                    have_auth = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_person", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auth",
                columns: table => new
                {
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<byte>(type: "smallint", nullable: false),
                    status = table.Column<byte>(type: "smallint", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    hash = table.Column<string>(type: "text", nullable: true),
                    salt = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth", x => x.person_id);
                    table.ForeignKey(
                        name: "FK_auth_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "confirm_code",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    date_create = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirm_code", x => x.id);
                    table.ForeignKey(
                        name: "FK_confirm_code_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lesson",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    trainer_id = table.Column<long>(type: "bigint", nullable: false),
                    duration_minute = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson", x => x.id);
                    table.ForeignKey(
                        name: "FK_lesson_person_trainer_id",
                        column: x => x.trainer_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_token_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sold_abonnement",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    date_sale = table.Column<long>(type: "bigint", nullable: false),
                    date_expiration = table.Column<long>(type: "bigint", nullable: false),
                    price_sold = table.Column<float>(type: "real", nullable: false),
                    visit_counter = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    validity = table.Column<byte>(type: "smallint", nullable: false),
                    number_of_visits = table.Column<byte>(type: "smallint", nullable: false),
                    base_price = table.Column<float>(type: "real", nullable: false),
                    discount = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sold_abonnement", x => x.id);
                    table.ForeignKey(
                        name: "FK_sold_abonnement_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "abonnements_lessons",
                columns: table => new
                {
                    abonnement_id = table.Column<long>(type: "bigint", nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abonnements_lessons", x => new { x.abonnement_id, x.lesson_id });
                    table.ForeignKey(
                        name: "FK_abonnements_lessons_abonnement_abonnement_id",
                        column: x => x.abonnement_id,
                        principalTable: "abonnement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_abonnements_lessons_lesson_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lesson",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sold_abonnements_lessons",
                columns: table => new
                {
                    abonnement_id = table.Column<long>(type: "bigint", nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sold_abonnements_lessons", x => new { x.lesson_id, x.abonnement_id });
                    table.ForeignKey(
                        name: "FK_sold_abonnements_lessons_lesson_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lesson",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sold_abonnements_lessons_sold_abonnement_abonnement_id",
                        column: x => x.abonnement_id,
                        principalTable: "sold_abonnement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_abonnement_name",
                table: "abonnement",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_abonnements_lessons_lesson_id",
                table: "abonnements_lessons",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_email",
                table: "auth",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_confirm_code_person_id",
                table: "confirm_code",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_confirm_code_token",
                table: "confirm_code",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lesson_name",
                table: "lesson",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lesson_trainer_id",
                table: "lesson",
                column: "trainer_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_person_id",
                table: "refresh_token",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_name",
                table: "room",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sold_abonnement_person_id",
                table: "sold_abonnement",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_sold_abonnements_lessons_abonnement_id",
                table: "sold_abonnements_lessons",
                column: "abonnement_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abonnements_lessons");

            migrationBuilder.DropTable(
                name: "auth");

            migrationBuilder.DropTable(
                name: "confirm_code");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "sold_abonnements_lessons");

            migrationBuilder.DropTable(
                name: "abonnement");

            migrationBuilder.DropTable(
                name: "lesson");

            migrationBuilder.DropTable(
                name: "sold_abonnement");

            migrationBuilder.DropTable(
                name: "person");
        }
    }
}
