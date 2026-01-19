namespace Contracts.Enums;

public enum OperationType
{
    ActualCosts = 1,              // 1) Дт20 Кт10 (ввод затрат)
    ReceiptFromProduction = 2,    // 2) Дт43 Кт20 (выпуск/поступление)
    Sale = 3,                     // 3) Реализация (Дт90 Кт43 + Дт62 Кт90)
    AllocateActualCost = 4,       // 4) Распределение факта (Дт43 Кт20, без количества)
    WriteOffDeviations = 5        // 5) Списание отклонений на 90 (Дт90 Кт43, без количества)
}
    