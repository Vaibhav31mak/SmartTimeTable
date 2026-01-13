export interface TimetableEntry {
  id: number;
  day: number; // 1 = Monday, 5 = Friday
  slot: string; // "09:00:00 - 10:00:00"
  subject: string;
  teacher: string;
  room: string;
  batchId: number;
}

export interface TimetableGridSlot {
  time: string;
  monday?: TimetableEntry;
  tuesday?: TimetableEntry;
  wednesday?: TimetableEntry;
  thursday?: TimetableEntry;
  friday?: TimetableEntry;
}