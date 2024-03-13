export class Note {
id?: number;
noteContent?: string;
sender?: number;
subjectId?: number;
timestamp?: string;

constructor(init?: Partial<Note>) {
  Object.assign(this,  init);
}
}

export class Journalist {
  journalistId?: number;
  username?: string;

  constructor(init?: Partial<Journalist>) {
    Object.assign(this, init);
  }

}

export class Subject {
  subjectId?: number;

  constructor(init?: Partial<Subject>) {
    Object.assign(this, init);
  }
}

export enum SubjectEnums{
  general,
  shopping,
  school
}
