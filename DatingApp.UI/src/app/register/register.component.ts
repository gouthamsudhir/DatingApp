import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  model: any = {};
  usersFromHomeComponent = input.required<any>();
  cancelRegister = output();
  private accountService: AccountService = inject(AccountService);

  register() {
    this.accountService.register(this.model).subscribe({
      next: () => this.cancel(),
      error: error => console.log(error)
    });
  }

  cancel() {
    this.model={};
    this.cancelRegister.emit();
  }
}
