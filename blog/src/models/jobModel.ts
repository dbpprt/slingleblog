
module Application {
    export class JobModel {
        jobId: string;
        jobName: string;
        description: string[];
        interval: string;
        nextScheduledExecution: string;
    }
}